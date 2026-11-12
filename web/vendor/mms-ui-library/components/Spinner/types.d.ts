export type SpinnerFillProps = {
    color?: string;
    animationDuration: number;
};
export type SpinnerProps = {
    size?: SpinnerSize;
    color?: string;
    animationDuration?: number;
    dataTestId?: string;
};
export type SpinnerSize = 'small' | 'medium';
export type SpinnerConfig = {
    width: number;
    height: number;
    path: string;
};
export type SpinnerFillWrapperProps = {
    width: number;
    height: number;
    path: string;
};
