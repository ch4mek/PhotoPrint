import {
	ProjectAssignFormDisableFlags,
	ProjectAssignFormOptions,
	ProjectAssignFormValues,
} from '../../types';

import { FormikWithBackupRenderProps } from '@/forms/components/FormikWithBackup/types';

export interface ProjectAccessAddingFormContentProps {
	formikProps: FormikWithBackupRenderProps<ProjectAssignFormValues>;
	formDisableFlags: ProjectAssignFormDisableFlags;
	options: ProjectAssignFormOptions;
	label: string;
	isLoading: boolean;
	isAssignEditable: boolean;
	onClose: VoidFunction;
	projectSubValueField?: string;
}
