import axios from 'axios';

import { LOGOUT_URL } from '@/constants/queryPaths';
import { useQueryWithCallback } from '@/helpers/useQueryWithCallback';

export const useLogout = () =>
	useQueryWithCallback({
		queryKey: [],
		queryFn: () => axios.get(LOGOUT_URL).then((res) => res.data),
		enabled: false,
	});
