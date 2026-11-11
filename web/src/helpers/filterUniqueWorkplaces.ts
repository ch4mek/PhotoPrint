import { Workplace } from '@/types/booking';

export function filterUniqueWorkplaces(workplaces: Workplace[]) {
	const uniqueWorkplaces: Workplace[] = [];
	const seenWorkplaceNumbers = new Set<number>();

	workplaces.forEach((workplace) => {
		if (!seenWorkplaceNumbers.has(workplace.workplaceNumber)) {
			uniqueWorkplaces.push(workplace);
			seenWorkplaceNumbers.add(workplace.workplaceNumber);
		}
	});

	return uniqueWorkplaces;
}
