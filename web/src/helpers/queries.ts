interface GetKeywordQueryOptions {
	search?: string | null;
	roles?: Array<string>;
	isPublic?: boolean;
	isActive?: boolean;
	isDeleted?: boolean;
	onlyManagedClients?: boolean;
	onlyManagedProjects?: boolean;
}

export const getKeywordQuery = ({
	search,
	roles,
	isPublic,
	isActive,
	isDeleted,
	onlyManagedClients,
	onlyManagedProjects,
}: GetKeywordQueryOptions) => {
	const queryParams: string[] = [];

	if (roles && roles.length > 0) {
		queryParams.push(`roles=${roles.join('&roles=')}`);
	}

	if (search) {
		queryParams.push(`keyWord=${search}`);
	}

	if (isActive !== undefined) {
		queryParams.push(`isActive=${isActive}`);
	}

	if (isPublic !== undefined) {
		queryParams.push(`isPublic=${!!isPublic}`);
	}

	if (isDeleted !== undefined) {
		queryParams.push(`isDeleted=${!!isDeleted}`);
	}

	if (onlyManagedClients) {
		queryParams.push(`onlyManagedClients=${onlyManagedClients}`);
	}

	if (onlyManagedProjects) {
		queryParams.push(`onlyManagedProjects=${onlyManagedProjects}`);
	}

	return queryParams.length ? `?${queryParams.join('&')}` : '';
};
