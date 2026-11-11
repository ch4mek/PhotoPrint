import { SoftSkill } from '@/types/settings';

export interface SoftSkillsFormProps {
	initialValues: SoftSkillsFormValues;
	handleClose: () => void;
	onSubmit: (values: SoftSkill) => Promise<object | undefined>;
	id: number | string;
}

export interface SoftSkillsFormValues {
	name: string;
}
