import { PersistentConfigurationItem } from '@/types/configuration';

export const getStatusDescription = (
	statuses: PersistentConfigurationItem[],
	status?: string
) => statuses.find(({ name }) => name === status)?.description ?? '';
