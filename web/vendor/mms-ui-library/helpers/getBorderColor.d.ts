export type BorderColor = 'graphics-accent' | 'graphics-error' | 'graphics-primary-2' | 'graphics-input-stroke';
export declare const getBorderColor: (error?: boolean, isFocused?: boolean, disabled?: boolean, isHover?: boolean) => BorderColor;
