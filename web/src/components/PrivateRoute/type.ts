import { Roles } from '@/constants/roles';
import { UserClaims } from '@/types/claims';

export interface PrivateRouteProps {
	available?: Roles[];
	redirectPath?: string;
	availablePermissions?: UserClaims[];
}
