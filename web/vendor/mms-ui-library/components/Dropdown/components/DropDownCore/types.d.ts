import { type DropTabTypography, DropTabValuesOptions } from '@/components/DropTab';
export interface ContentProps {
    arrowAndTextAlign: 'center' | 'space-between' | 'space-around' | 'end' | 'start';
    verticalPaddings: number;
    width?: 'medium' | 'small' | 'large' | 'fullWidth';
}
export interface DropDownCoreProps extends Pick<DropTabValuesOptions, 'nameField' | 'valueField'>, Partial<ContentProps> {
    value: any;
    options: any[];
    width?: 'medium' | 'small' | 'large' | 'fullWidth';
    dataTestId?: string;
    outlined?: boolean;
    disabled?: boolean;
    typography?: DropTabTypography;
    mainTextColor?: string;
    showToolTipIfCut?: boolean;
    isArrowButtonEnabled?: boolean;
    valueLabelOverride?: string;
    error?: boolean;
    errorText?: string;
    isActive?: boolean;
}
