import {
	createContext,
	useMemo,
	useContext,
	PropsWithChildren,
	useState,
} from 'react';

import { defaultConfiguration } from '@/constants/defaultValues';

import type { ConfigurationContextValues, ConfigurationState } from './types';

const ConfigurationContext = createContext({} as ConfigurationContextValues);

export function ConfigurationProvider({ children }: PropsWithChildren) {
	const [configurationContextValue, setConfigurationContextValue] =
		useState<ConfigurationState>({
			...defaultConfiguration,
			isLoading: false,
		});

	const contextValue: ConfigurationContextValues = useMemo(
		() => ({
			...(configurationContextValue || defaultConfiguration),
			setConfigurationContextValue,
		}),
		[configurationContextValue]
	);

	return (
		<ConfigurationContext.Provider value={contextValue}>
			{children}
		</ConfigurationContext.Provider>
	);
}

export const useConfiguration = () => useContext(ConfigurationContext);
