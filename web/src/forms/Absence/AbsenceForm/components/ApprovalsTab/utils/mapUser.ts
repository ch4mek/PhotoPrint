export const mapUser = (id?: string | number | null, name?: string | null) =>
	id && name ? { id, name } : null;
