import { EquipmentFormsChanges } from '../types';

export interface FormDirtyTrackerProps {
	formKey: keyof EquipmentFormsChanges;
}
