export interface UserSpecializationFormProps {
	handleClose: () => void;
	userId: number | undefined;
	primarySpecializationId?: number;
}

export interface SpecializationTouched {
	specialization?: {
		seniority?: boolean;
		hardSkills: boolean[];
	};
}
