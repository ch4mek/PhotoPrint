export interface PaletteControlProps {
    palette: string[];
    value: string;
    handleDeleteColor: VoidFunction;
    selectedColor: string | null;
    dataTestId: string;
    onPaletteCreate?: (color: string) => void;
}
