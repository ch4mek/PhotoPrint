import { PhotoProps } from '../Photo';
export interface AvatarProps extends Omit<PhotoProps, 'firstName' | 'lastName'> {
    src?: string;
    firstName?: string;
    lastName?: string;
}
