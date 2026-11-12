import React, { PropsWithChildren } from 'react';
import { DropDownProps } from '@/components/Dropdown/types';
type DropdownContainerProps = PropsWithChildren<DropDownProps> & {
    isActive: boolean;
    onToggle: () => void;
    borderColor: string;
    mainRef: React.RefObject<HTMLDivElement | null>;
    popoverRef: React.RefObject<HTMLDivElement | null>;
    onSelect: (v: unknown) => void;
};
export declare function DropdownContainer({ options, value, valueField, dataTestId, height, outlined, disabled, horizontalPaddings, onChange, hasSelected, dropTabPaddings, minItemHeight, gapBetweenItems, paddingInsideItemStyle, dropTabWidth, renderOnTop, className, stylesConfig, preventDropdownFlip, getKey, useAnchorWidth, isActive, onToggle, borderColor, mainRef, popoverRef, onSelect, children, width, ...optionProps }: DropdownContainerProps): import("react/jsx-runtime").JSX.Element;
export {};
