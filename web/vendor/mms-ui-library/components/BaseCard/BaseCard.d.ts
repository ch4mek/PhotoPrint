import { BaseCardBodyProps, BaseCardHeaderProps, BaseCardRowProps, BaseCardWrapperProps } from './types';
declare function BaseCardWrapper({ children, className, maxWidth, maxHeight, minWidth, isFullWidth, dataTestId, ...props }: BaseCardWrapperProps): import("react/jsx-runtime").JSX.Element;
declare function BaseCardHeader({ children, className, color, element, dataTestId, ...props }: BaseCardHeaderProps): import("react/jsx-runtime").JSX.Element;
declare function BaseCardBody({ children, className, color, hoverColor, padding, dataTestId, ...props }: BaseCardBodyProps): import("react/jsx-runtime").JSX.Element;
declare function BaseCardRow({ children, className, dataTestId, ...props }: BaseCardRowProps): import("react/jsx-runtime").JSX.Element;
export declare const BaseCard: {
    Wrapper: typeof BaseCardWrapper;
    Header: typeof BaseCardHeader;
    Body: typeof BaseCardBody;
    Row: typeof BaseCardRow;
};
export {};
