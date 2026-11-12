import { ReactNode } from 'react';
import { CheckIfOptionEqualToValue, GetKey } from '@/types/common';
import type { BodyFontSize } from '@/types/emotion';
export interface DropTabValuesOptions {
    valueField?: string;
    subValueField?: string;
    groupByField?: string;
    iconField?: string;
    nameField?: string;
    subValueLinesCount?: number;
}
export interface DropTabTypography {
    valueTypography?: BodyFontSize;
    subvalueTypography?: BodyFontSize;
    headerTypography?: BodyFontSize;
}
export interface DropTabProps<T> extends DropTabWidthProps, DropTabValuesOptions {
    options: T[];
    value: T;
    onChange: (option: T) => void;
    multipleChoice?: boolean;
    allowEmpty?: boolean;
    dataTestId?: string;
    minItemHeight?: number;
    maxHeight?: number;
    paddingInsideItemStyle?: string;
    noOptionsText?: string;
    checkIfChipDisabled?: (option: T) => boolean;
    checkIfOptionEqualToValue?: CheckIfOptionEqualToValue<T>;
    getKey?: GetKey<T>;
    handleChangeEndPage?: () => void;
    handleChangeStartPage?: () => void;
    typography?: DropTabTypography;
    dropTabItemSpacing?: number;
    renderCreateRow?: ReactNode;
}
export interface DropTabPaddingsProps {
    paddings?: number;
}
export interface DropTabWidthProps extends DropTabPaddingsProps {
    width?: string;
}
export interface ItemProps {
    isHeader: boolean;
    isSelected: boolean;
    minItemHeight: number;
    paddingInsideItemStyle: string;
    dropTabItemSpacing?: number;
}
