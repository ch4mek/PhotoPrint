export const getSeniorityAndSpecialization = (
	seniority: string | null,
	specialization: string | null
) => {
	if (!seniority || !specialization) {
		return '-';
	}

	return `${seniority} (${specialization})`;
};
