import { ProjectAssignFormValues } from '@/forms/ProjectAccessAddingBase/types';
import { FormUserAutoselectOption } from '@/types/forms';

export interface TeamboardUserProjectsFormProps {
	handleClose: () => void;
	onSubmit: (values: ProjectAssignFormValues) => void;
	projectAssignId?: number;
	showLoader?: boolean;
	user: FormUserAutoselectOption;
}
