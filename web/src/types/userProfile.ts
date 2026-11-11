interface ShortHardSkillInfo {
	id: number;
	name: string;
	isActive: boolean;
}

export interface HardSkill {
	id: number;
	name: string;
	seniorityLevelId: string;
	seniorityLevelName: string;
	isActive: boolean;
}

export interface SpecializationSnapshot {
	id: number;
	userId: number;
	snapshotDate: string;
	userSpecializations: UserSpecialization[];
}

export interface UserSpecializationInfo {
	id: number;
	seniorityLevelId: string;
	seniorityLevelName: string;
	isPrimary: boolean;
	specializationTemplateId: number;
	specializationTemplateName: string;
	specializationSnapshots: SpecializationSnapshot[];
}

export interface UserSpecialization {
	id: number;
	seniorityLevelId: string;
	seniorityLevelName: string;
	wasPrimary: boolean;
	specializationTemplateId: number;
	specializationTemplateName: string;
	hardSkills: HardSkill[];
}

export interface FullHardSkillInfo extends ShortHardSkillInfo {
	seniorityLevelId: string;
	seniorityLevelName: string;
}
