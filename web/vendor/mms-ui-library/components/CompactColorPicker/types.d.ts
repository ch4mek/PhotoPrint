import { ReactNode } from 'react';
export interface CompactColorPickerProps {
    palette: string[];
    value: string | null;
    onChange: (value: string) => void;
    dataTestId?: string;
    controls?: ReactNode | null;
    showPaletteTitle?: boolean;
    paletteTitle?: string;
}
