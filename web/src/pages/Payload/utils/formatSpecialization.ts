export const formatSpecialization = (
	specialization?: string | null,
	seniority?: string
) => {
	if (!specialization) {
		return '';
	}

	return seniority ? `${specialization} (${seniority})` : specialization;
};
