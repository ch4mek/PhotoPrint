import { createContext } from 'react';

import { GroupInfoContextValues } from '../types';

const initialGroupInfoContextValue: GroupInfoContextValues = {
	departmentId: '',
	groupId: -1,
};

export const GroupInfoContext = createContext<GroupInfoContextValues>(
	initialGroupInfoContextValue
);
