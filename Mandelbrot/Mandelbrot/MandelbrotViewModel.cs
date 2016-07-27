using System;
using System.Threading;
using System.Windows.Input;         
using Xamarin.Forms;

namespace Mandelbrot
{
    class MandelbrotViewModel : ViewModelBase
    {
        readonly double baseWidth;
        readonly double baseHeight;
        
        Complex currentCenter, targetCenter;
        int pixelWidth, pixelHeight;
        double currentMagnification, targetMagnification;
        int iterations;
        double realOffset, imaginaryOffset;
        bool isBusy;
        double progress;
        BitmapInfo bitmapInfo;

        public MandelbrotViewModel(double baseWidth, double baseHeight)
        {
            this.baseWidth = baseWidth;
            this.baseHeight = baseHeight;
            
            MandelbrotModel model = new MandelbrotModel();
            
            Progress<double> progressReporter = new Progress<double>((double progress) =>
            {
                Progress = progress;
            });

            CancellationTokenSource cancelTokenSource = null;
            
            CalculateCommand = new Command(
                execute: async () =>
                {
                    IsBusy = true;
                    ((Command)CalculateCommand).ChangeCanExecute();
                    ((Command)CancelCommand).ChangeCanExecute();
                    
                    cancelTokenSource = new CancellationTokenSource();
                    CancellationToken cancelToken = cancelTokenSource.Token;

                    try
                    {
                        BitmapInfo = await model.CalculateAsync(TargetCenter,
                                                                baseWidth / TargetMagnification,
                                                                baseHeight / TargetMagnification,
                                                                PixelWidth, PixelHeight,
                                                                Iterations,
                                                                progressReporter,
                                                                cancelToken);
                        
                        CurrentCenter = TargetCenter;
                        CurrentMagnification = TargetMagnification;
                        RealOffset = 0.5;
                        ImaginaryOffset = 0.5;
                    }
                    catch (OperationCanceledException)
                    {
                    }
                    catch
                    {
                    }
                    
                    Progress = 0;
                    IsBusy = false;
                    
                    ((Command)CalculateCommand).ChangeCanExecute();
                    ((Command)CancelCommand).ChangeCanExecute();
                },
                canExecute: () =>
                {
                    return !IsBusy;
                });

            CancelCommand = new Command(
                execute: () =>
                {
                    cancelTokenSource.Cancel();
                },
                canExecute: () =>
                {
                    return IsBusy;
                });
        }

        public int PixelWidth
        {
            set { SetProperty(ref pixelWidth, value); }
            get { return pixelWidth; }
        }

        public int PixelHeight
        {
            set { SetProperty(ref pixelHeight, value); }
            get { return pixelHeight; }
        }

        public Complex CurrentCenter
        {
            set
            {
                if (SetProperty(ref currentCenter, value))
                    CalculateTargetCenter();
            }
            get { return currentCenter; }
        }

        public Complex TargetCenter
        {
            private set { SetProperty(ref targetCenter, value); }
            get { return targetCenter; }
        }

        public double CurrentMagnification
        {
            set { SetProperty(ref currentMagnification, value); }
            get { return currentMagnification; }
        }

        public double TargetMagnification
        {
            set { SetProperty(ref targetMagnification, value); }
            get { return targetMagnification; }
        }

        public int Iterations
        {
            set { SetProperty(ref iterations, value); }
            get { return iterations; }
        }
        
        public double RealOffset
        {
            set
            {
                if (SetProperty(ref realOffset, value))
                    CalculateTargetCenter();
            }
            get { return realOffset; }
        }

        public double ImaginaryOffset
        {
            set
            {
                if (SetProperty(ref imaginaryOffset, value))
                    CalculateTargetCenter();
            }
            get { return imaginaryOffset; }
        }

        void CalculateTargetCenter()
        {
            double width = baseWidth / CurrentMagnification;
            double height = baseHeight / CurrentMagnification;

            TargetCenter = new Complex(CurrentCenter.Real + (RealOffset - 0.5) * width,
                                       CurrentCenter.Imaginary + (ImaginaryOffset - 0.5) * height);
        }

        public bool IsBusy
        {
            private set { SetProperty(ref isBusy, value); }
            get { return isBusy; }
        }

        public double Progress
        {
            private set { SetProperty(ref progress, value); }
            get { return progress; }
        }

        public BitmapInfo BitmapInfo
        {
            private set { SetProperty(ref bitmapInfo, value); }
            get { return bitmapInfo; }
        }

        public ICommand CalculateCommand { private set; get; }

        public ICommand CancelCommand { private set; get; }
    }
}