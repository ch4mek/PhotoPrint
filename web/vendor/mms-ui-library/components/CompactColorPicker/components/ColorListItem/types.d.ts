export interface ColorListItemProps {
    color: string;
    onClick?: (color: string) => void;
    selected?: boolean;
    dataTestId: string;
}
