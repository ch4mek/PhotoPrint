import { Roles } from '@/constants/roles';
import { RoutesType } from '@/constants/routes';
import { UserClaims } from '@/types/claims';

export interface ItemPayload {
	slug: RoutesType[keyof RoutesType];
	roles: Roles[];
	claims: UserClaims[];
}
