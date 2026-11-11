import { ShortHardSkillInfo, UserHardSkillLookup } from '@/types/hardSkills';

export interface HardSkillsProps {
	hardSkills: UserHardSkillLookup<ShortHardSkillInfo>[] | undefined;
}

export interface HardSkillsSectionRow {
	id: number;
	isPrimary: boolean;
	seniority: string;
	name: string;
}
