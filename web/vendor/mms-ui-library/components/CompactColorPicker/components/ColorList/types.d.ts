export interface ColorListProps {
    items: string[];
    onSelect?: (color: string) => void;
    selectedColor?: string | null;
    dataTestId?: string;
}
