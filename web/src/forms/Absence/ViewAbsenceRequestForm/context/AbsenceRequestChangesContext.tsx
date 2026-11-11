import {
	createContext,
	PropsWithChildren,
	useContext,
	useMemo,
	useState,
} from 'react';

import { AbsenceRequestChangesContextValues } from '../types';

const AbsenceRequestChangesContext =
	createContext<AbsenceRequestChangesContextValues>(
		{} as AbsenceRequestChangesContextValues
	);

export function AbsenceRequestChangesProvider({ children }: PropsWithChildren) {
	const [hasUnsavedChanges, setHasUnsavedChanges] = useState(false);

	const contextValue: AbsenceRequestChangesContextValues = useMemo(
		() => ({
			hasUnsavedChanges,
			setHasUnsavedChanges,
		}),
		[hasUnsavedChanges]
	);

	return (
		<AbsenceRequestChangesContext value={contextValue}>
			{children}
		</AbsenceRequestChangesContext>
	);
}

export const useAbsenceRequestChanges = () =>
	useContext(AbsenceRequestChangesContext);
