export const formatDiscountPercent = (
	minPercent?: number | null,
	maxPercent?: number | null
) => {
	if (minPercent && maxPercent) {
		return `${minPercent}-${maxPercent}%`;
	}

	if (minPercent || maxPercent) {
		return `${minPercent ?? maxPercent}%`;
	}

	return '';
};
