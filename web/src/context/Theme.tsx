import { css, Global } from '@emotion/react';
import { createTheme, ThemeProvider } from '@mui/material/styles';
import emotionNormalize from 'emotion-normalize';
import {
	createContext,
	PropsWithChildren,
	useCallback,
	useContext,
	useMemo,
	useState,
} from 'react';

import { THEME_SCHEME_STORAGE_KEY } from '@/constants/storageKeys';
import { getDesignTokens, ThemeMode } from '@/theme';

import { ColorModeContextValues } from './types';

const ThemeModeContext = createContext({
	mode: ThemeMode.DARK,
} as ColorModeContextValues);

const globalStyles = css`
	${emotionNormalize}

	html {
		box-sizing: border-box;
		-webkit-font-smoothing: antialiased;
	}

	*,
	::before,
	::after {
		box-sizing: inherit;
	}
`;

export function ColorSchemeProvider({ children }: PropsWithChildren) {
	const [selectedMode, setMode] = useState<ThemeMode>(() => {
		const currentTheme = localStorage.getItem(THEME_SCHEME_STORAGE_KEY);
		const initialValue = currentTheme
				? JSON.parse(currentTheme)
				: ThemeMode.DARK;

		return initialValue;
	});

	const toggleThemeMode = useCallback(() => {
		setMode((snapshot) => {
			const nextMode =
				snapshot === ThemeMode.LIGHT ? ThemeMode.DARK : ThemeMode.LIGHT;

			localStorage.setItem(THEME_SCHEME_STORAGE_KEY, JSON.stringify(nextMode));

			return nextMode as ThemeMode;
		});
	}, [selectedMode]);

	const colorScheme = useMemo(
		() => createTheme(getDesignTokens(selectedMode)),
		[selectedMode]
	);

	const contextValue = useMemo(
		() => ({
			mode: selectedMode,
			toggleThemeMode,
			theme: colorScheme,
		}),
		[selectedMode, toggleThemeMode]
	);

	return (
		<ThemeModeContext.Provider value={contextValue}>
			<ThemeProvider theme={colorScheme}>
				<Global styles={globalStyles} />
				{children}
			</ThemeProvider>
		</ThemeModeContext.Provider>
	);
}

export const useColorMode = () => useContext(ThemeModeContext);
