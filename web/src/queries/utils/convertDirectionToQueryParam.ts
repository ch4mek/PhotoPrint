import { AttendenceDirection } from '@/types/attendances';

export const convertDirectionToQueryParam = (
	directions: AttendenceDirection[]
) => {
	if (!directions.length) {
		return undefined;
	}

	return directions.map((d) =>
		d === AttendenceDirection.Enter
			? AttendenceDirection.Enter
			: AttendenceDirection.Exit
	);
};
