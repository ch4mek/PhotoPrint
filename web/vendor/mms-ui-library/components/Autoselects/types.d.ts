import { BorderColor } from '@/helpers/getBorderColor';
import type { BodyFontSize } from '@/types/emotion';
export interface WrapperProps {
    width: number | '100%';
    disabled?: boolean;
    stylesConfig?: Omit<StylesConfig, 'typography'>;
    isOpen?: boolean;
}
export interface StylesConfig {
    backgroundColor?: string;
}
export interface ContainerProps {
    isValueSelected: boolean;
    disabled?: boolean;
    stylesConfig?: Omit<StylesConfig, 'typography'>;
    borderColor: BorderColor;
}
export interface ChipsContainerProps {
    isLimit?: boolean;
}
export declare enum AutoselectTypes {
    Small = "small",
    Medium = "medium"
}
export interface LabelProps {
    active?: boolean;
    disabled?: boolean;
    bgColor?: string;
    isAsPlaceholder?: boolean;
    error: boolean;
    labelTypography: BodyFontSize;
    activeLabelTypography: BodyFontSize;
}
