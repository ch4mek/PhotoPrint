import { useFormikContext } from 'formik';
import { useEffect } from 'react';

import { useEquipmentChangesContext } from '../context/EquipmentChangesContext';

import { FormDirtyTrackerProps } from './types';

export function FormDirtyTracker({ formKey }: FormDirtyTrackerProps) {
	const { dirty } = useFormikContext();
	const { setHasUnsavedChanges } = useEquipmentChangesContext();

	useEffect(() => {
		setHasUnsavedChanges((prev) => ({
			...prev,
			[formKey]: dirty,
		}));

		return () => {
			setHasUnsavedChanges((prev) => ({
				...prev,
				[formKey]: false,
			}));
		};
	}, [dirty, formKey]);

	return null;
}
