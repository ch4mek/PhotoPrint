import { createContext } from 'react';

import { ProjectLookupResponse } from '@/types/project';

export const PublicProjectContext = createContext<
	ProjectLookupResponse[] | undefined
>(undefined);
