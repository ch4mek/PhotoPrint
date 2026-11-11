import { GroupMutationType } from '@/types/department';
import { FormAutoselectOption } from '@/types/forms';

export interface GroupMutationFormValues {
	name: string;
	groupManager: FormAutoselectOption | null;
}

export interface GroupMutationFormProps {
	id?: number;
	departmentId?: string;
	handleClose: VoidFunction;
	refetch: VoidFunction;
	mutationType: GroupMutationType;
}
