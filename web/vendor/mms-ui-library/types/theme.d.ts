import type { ThemeMode, breakpoints, darkPalette, lightPalette, spaces, typography } from '@/theme';
export type Theme = {
    mode: ThemeMode;
    palette: typeof lightPalette | typeof darkPalette;
    typography: typeof typography;
    spaces: typeof spaces;
    breakpoints: typeof breakpoints;
};
