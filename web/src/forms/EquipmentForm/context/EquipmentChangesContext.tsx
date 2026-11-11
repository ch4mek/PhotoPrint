import {
	createContext,
	PropsWithChildren,
	useContext,
	useMemo,
	useState,
} from 'react';

import { EquipmentChangesContextValues, EquipmentFormsChanges } from '../types';

const EquipmentChangesContext = createContext<EquipmentChangesContextValues>(
	{} as EquipmentChangesContextValues
);

export function EquipmentChangesProvider({ children }: PropsWithChildren) {
	const [hasUnsavedChanges, setHasUnsavedChanges] =
		useState<EquipmentFormsChanges>({
			mainForm: false,
			userForm: false,
		});

	const contextValue: EquipmentChangesContextValues = useMemo(
		() => ({
			hasUnsavedChanges: Object.values(hasUnsavedChanges).some(
				(value) => value === true
			),
			setHasUnsavedChanges,
		}),
		[hasUnsavedChanges]
	);

	return (
		<EquipmentChangesContext value={contextValue}>
			{children}
		</EquipmentChangesContext>
	);
}

export const useEquipmentChangesContext = () =>
	useContext(EquipmentChangesContext);
