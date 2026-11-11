import { createContext } from 'react';

import { TabContextValues } from '../types';

const defaultContextValue: TabContextValues = {
	handleTabChange: () => {},
};

export const TabContext = createContext<TabContextValues>(defaultContextValue);
