export interface DropDownCalendarProps extends ContainerProps {
    options: string[];
    onClick: (option: string) => void;
    dataTestId?: string;
}
export interface ContainerProps {
    width?: string | number;
}
export interface ArrowButtonProps {
    isOpen: boolean;
}
