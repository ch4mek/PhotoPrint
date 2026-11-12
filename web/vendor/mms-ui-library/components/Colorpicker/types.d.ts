export interface ColorPickerProps {
    palette: string[];
    defaultValue?: string;
    onApply?: (color: string) => void;
    onPaletteDelete?: (color: string) => void;
    onPaletteCreate?: (color: string) => void;
    dataTestId?: string;
}
