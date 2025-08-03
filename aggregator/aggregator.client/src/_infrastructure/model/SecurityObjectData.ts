import { SecurityObjectClass} from './SecurityObjectClass';

export interface SecurityObjectData
{
    class: SecurityObjectClass;
    fullName: string;
    id: number;
    name: string;
    owner: number;
    ownerName: string;
    pid: number;
    childCount : number;

}