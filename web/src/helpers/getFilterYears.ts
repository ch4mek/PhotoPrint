import { generateYearsCompanyExistence } from '@/helpers/date';

export function getFilterYears() {
	const currentYear = new Date().getFullYear();
	const years = generateYearsCompanyExistence();

	return [currentYear + 1, ...years].map(String);
}
