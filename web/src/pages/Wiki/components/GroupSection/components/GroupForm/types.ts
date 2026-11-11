import { SegmentTypes } from '@/types/wiki';

export interface GroupDataFormProps {
	type: SegmentTypes;
	handleClose: VoidFunction;
	handleSubmit: (values: GroupFormValues) => Promise<object | undefined> | void;
	groupId?: number | string;
}

export interface GroupFormValues {
	name: string;
}
