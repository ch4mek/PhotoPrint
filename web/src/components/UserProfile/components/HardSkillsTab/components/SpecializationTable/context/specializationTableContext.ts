import { createContext, useContext } from 'react';

import { SpecializationTableContextValues } from '../types';

export const SpecializationTableContext =
	createContext<SpecializationTableContextValues>({
		allDates: [],
	});

export const useSpecializationTableContext = () =>
	useContext(SpecializationTableContext);
