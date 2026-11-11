import { GraduateJobPlacement } from '@/types/personalData';

export function boolToGraduateJobPlacement(
	value: boolean | null
): GraduateJobPlacement | null {
	if (value === null) {
		return null;
	}

	return value ? GraduateJobPlacement.Yes : GraduateJobPlacement.No;
}

export function graduateJobPlacementToBool(
	value: GraduateJobPlacement | null
): boolean | null {
	if (value === null) {
		return null;
	}

	return value === GraduateJobPlacement.Yes;
}
