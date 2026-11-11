import { LOGIC_OPTIONS } from '@/pages/Payload/contants';

export function getNeedProjectFilter(filter: string[]) {
	if (filter.length > 1 || !filter.length) {
		return null;
	}

	return filter[0] === LOGIC_OPTIONS[0].id;
}
