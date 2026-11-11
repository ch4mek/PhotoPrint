export type ProgrammerDayScoreRequest = {
	score: number;
};

export type ProgrammerDayResult = {
	avatarId: string | null;
	departmentName: string | null;
	firstName: string;
	firstNameRu: string | null;
	lastName: string;
	lastNameRu: string | null;
	score: number;
};
