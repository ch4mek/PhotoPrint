import { HardSkill } from '@/types/settings';

export interface HardSkillsFormProps {
	initialValues: HardSkillsFormValues;
	handleClose: () => void;
	onSubmit: (values: HardSkill) => Promise<object | undefined>;
	id: number | string;
}

export interface HardSkillsFormValues {
	name: string;
	hardSkillGroupId: HardSkill | null;
}
