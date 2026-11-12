import { CSSProperties, MouseEvent } from 'react';
import type { DropTabValuesOptions } from '../DropTab';
import { DropDownCoreProps } from '@/components/Dropdown/components/DropDownCore/types';
import { BorderColor } from '@/helpers/getBorderColor';
import { GetKey } from '@/types/common';
export interface SideActionConfig {
    id: string;
    label: string;
    onClick: (e: MouseEvent<HTMLButtonElement>) => void;
    disabled?: boolean;
    dataTestId?: string;
}
export interface DropDownProps extends Omit<DropDownCoreProps, 'isActive'>, Omit<DropTabValuesOptions, 'nameField' | 'valueField' | 'subValueLinesCount'>, Partial<MainProps> {
    onChange?: (value: any) => void;
    hasSelected?: boolean;
    dropTabPaddings?: number;
    minItemHeight?: number;
    gapBetweenItems?: number;
    paddingInsideItemStyle?: string;
    dropTabWidth?: string;
    closeOnSelect?: boolean;
    renderOnTop?: boolean;
    className?: string;
    preventDropdownFlip?: boolean;
    getKey?: GetKey<string | number>;
    useAnchorWidth?: boolean;
}
export interface MainProps extends Pick<DropDownCoreProps, 'width' | 'outlined' | 'disabled'> {
    height: string;
    horizontalPaddings: number;
    borderColor: BorderColor;
    stylesConfig?: CSSProperties;
}
