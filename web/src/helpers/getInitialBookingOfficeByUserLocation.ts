import { LocationTypeId, OfficeType } from '@/types/configuration';

import { sortOfficeItems } from './sortOfficeItems';

export const getInitialBookingOfficeByUserLocation = (
	offices: Array<OfficeType>,
	location: LocationTypeId
) => {
	const userOffice = offices.find((office) => office.location === location);

	if (userOffice) {
		return sortOfficeItems(userOffice);
	}

	if (
		location === LocationTypeId.Minsk ||
		location === LocationTypeId.Vitebsk
	) {
		const nearestOffice = offices.find(
			(office) => office.location === LocationTypeId.Minsk
		);

		if (nearestOffice) {
			return sortOfficeItems(nearestOffice);
		}
	}

	if (
		location === LocationTypeId.Warsaw ||
		location === LocationTypeId.Tbilisi
	) {
		const nearestOffice = offices.find(
			(office) => office.location === LocationTypeId.Warsaw
		);

		if (nearestOffice) {
			return sortOfficeItems(nearestOffice);
		}
	}

	return null;
};
