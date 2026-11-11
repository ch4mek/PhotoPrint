const ISO_DATE_FRACTION_REGEX = /\.(\d+)Z$/;

export const normalizeIsoDate = (isoStringDateFormat: string) =>
	isoStringDateFormat.replace(
		ISO_DATE_FRACTION_REGEX,
		(_, fraction) => `.${fraction.padEnd(9, '0')}Z`
	);
