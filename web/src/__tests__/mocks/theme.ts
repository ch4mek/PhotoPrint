import { Theme } from '@emotion/react';
import { createTheme } from '@mui/material';

import { ThemeMode, getDesignTokens } from '@/theme';

export const mockTheme: Theme = createTheme(getDesignTokens(ThemeMode.LIGHT));
