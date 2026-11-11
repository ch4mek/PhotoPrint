export function parsePositiveId(
	id: string | null | undefined | number
): number {
	if (id == null || id === '') {
		return 0;
	}
	const parsed = Number(id);

	return Number.isFinite(parsed) && parsed > 0 ? Math.floor(parsed) : 0;
}
