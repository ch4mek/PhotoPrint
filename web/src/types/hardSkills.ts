export interface UserHardSkillLookup<
	T extends FullHardSkillInfo | ShortHardSkillInfo
> {
	id: number;
	isPrimary: boolean;
	seniorityLevelId: string;
	specializationTemplateId: number;
	specializationTemplateName: string;
	hardSkills: Array<T>;
	seniorityLevelName: string;
	userId: number;
}

export interface ShortHardSkillInfo {
	id: number;
	name: string;
	isActive: boolean;
}

export interface FullHardSkillInfo extends ShortHardSkillInfo {
	seniorityLevelId: string;
	seniorityLevelName: string;
}
